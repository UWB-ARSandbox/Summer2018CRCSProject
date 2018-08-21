
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
import javafx.application.Application;
import javafx.event.ActionEvent;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.layout.StackPane;
import javafx.scene.layout.VBox;
import javafx.stage.DirectoryChooser;
import javafx.stage.Stage;

/**
 * ScanRenamer is a class that you select a starting folder and ending folder
 * and it extracts zip files from the starting directory and outputs the renamed
 * files into the ending directory.
 *
 * @author Ben Clark
 */
public class ScanRenamer extends Application {

    Stage primaryStage;
    Label l;
    byte[] buffer;

    /**
     * Sets up the frame. Adds in the button and some labels.
     *
     * @param primaryStage The main frame that is used throughout.
     */
    @Override
    public void start(Stage primaryStage) {
        buffer = new byte[1024];
        this.primaryStage = primaryStage;
        l = new Label();
        Button btn = new Button();
        btn.setText("Pick zip file");
        btn.setOnAction((ActionEvent e) -> {
            buttonPressed();
        });
        VBox componentStack = new VBox();
        componentStack.setAlignment(Pos.CENTER);
        componentStack.getChildren().addAll(btn, l);
        StackPane root = new StackPane();
        root.getChildren().add(componentStack);

        Scene scene = new Scene(root, 200, 150);

        primaryStage.setTitle("File Renamer");
        primaryStage.setScene(scene);
        primaryStage.show();
    }

    /**
     * Once the button has been pressed, prompts the user to get a starting and
     * ending directory. It then goes through all the files in the starting
     * directory and processes them. Once done it sets the text to done.
     */
    private void buttonPressed() {
        File inDir = getDir("Starting Directory");
        if (inDir == null) {
            return;
        }
        File outDir = getDir("Ending Directory");
        if (outDir == null) {
            return;
        }

        File[] inFiles = inDir.listFiles();
        for (File inFile : inFiles) {
            processFile(inFile, outDir);
        }

        l.setText("Done");
    }

    /**
     * The main function for processing files. It takes in a file (assumed to be
     * zipped) and proceeds to rename it and extract it. The renaming is done to
     * standardize the file ending. Once the file has been extracted it is
     * deleted. The remaining directory of the extracted files is then
     * processed. The internal files are then renamed and the object and
     * material files are handled. Finally all the files are moved into the
     * ending directory.
     *
     * @param selectedFile The zip file to be processed.
     * @param outDir The directory to put the processed files into.
     */
    private void processFile(File selectedFile, File outDir) {
        String selectedFileName
                = selectedFile.getName().substring(
                        0, selectedFile.getName().lastIndexOf("."));
        File outname = new File("test.zip");
        selectedFile.renameTo(outname);

        File dir = new File(selectedFileName);
        dir.mkdir();

        try {
            unzip(outname.getName(), selectedFileName);
        } catch (Exception e) {
            System.out.println("ERROR in Unzip");
            return;
        }

        outname.delete();
        selectedFile.delete();

        File[] files = dir.listFiles();

        for (File f : files) {
            String name = f.getName();
            String extension = name.substring(name.length() - 4);
            File out = new File(selectedFileName + extension);
            f.renameTo(out);
            try {
                if (extension.equals(".obj")) {
                    editObjLine(out.toPath(), selectedFileName);
                } else if (extension.equals(".mtl")) {
                    editMtlLine(out.toPath(), selectedFileName);
                }
            } catch (IOException ex) {
                System.out.println("ERROR in fileEdits");
                return;
            }

            try {
                Path temp = Files.move(Paths.get(out.getName()),
                        Paths.get(outDir.getAbsolutePath() + "\\"
                                + out.getName()));
                if (temp == null) {
                    System.out.println("Failed to move the file");
                    return;
                }
            } catch (IOException ex) {
                System.out.println("Failed to move the file");
                return;
            }
            f.delete();
            out.delete();
        }
        dir.delete();
    }

    /**
     * Gives a pop-up file chooser targeted at directories.
     *
     * @param title The title for the file chooser
     * @return The choice directory or null if they quit the file chooser.
     */
    private File getDir(String title) {
        DirectoryChooser fileChooser = new DirectoryChooser();
        fileChooser.setTitle(title);
        return fileChooser.showDialog(primaryStage);
    }

    /**
     * Un-zips the file with the given name fileZip into the folder with the
     * name folderName
     *
     * @param fileZip The name of the zipped folder.
     * @param folderName The name of the folder to extract the files into
     * @throws IOException If there are any issues with the files
     */
    private void unzip(String fileZip, String folderName)
            throws IOException {
        for (int i = 0; i < buffer.length; i++) {
            buffer[i] = 0;
        }
        try (ZipInputStream zis
                = new ZipInputStream(new FileInputStream(fileZip))) {
            ZipEntry zipEntry = zis.getNextEntry();
            while (zipEntry != null) {
                String fileName = zipEntry.getName();
                File newFile = new File(folderName + "/" + fileName);
                try (FileOutputStream fos = new FileOutputStream(newFile)) {
                    for (int i = zis.read(buffer); i > 0; zis.read(buffer)) {
                        fos.write(buffer, 0, i);
                    }
                }
                zipEntry = zis.getNextEntry();
            }
            zis.closeEntry();
        }
    }

    /**
     * Reads the file into memory, then finds the issue line with the old name
     * and changes it. Then writes the file back to memory.
     *
     * @param filePath The path for the file.
     * @param newFileName The new name for the file.
     * @throws IOException If there are any issues with the file
     */
    private void editMtlLine(Path filePath, String newFileName)
            throws IOException {
        ArrayList<String> fileContent
                = new ArrayList<>(Files.readAllLines(filePath,
                        StandardCharsets.UTF_8));

        for (int i = 0; i < fileContent.size(); i++) {
            if (fileContent.get(i).equals("map_Kd Model.jpg")) {
                fileContent.set(i, "map_Kd " + newFileName + ".jpg");
                break;
            }
        }

        Files.write(filePath, fileContent, StandardCharsets.UTF_8);
    }

    /**
     * Reads the file into memory, then finds the issue line with the old name
     * and changes it. Then writes the file back to memory.
     *
     * @param filePath The path for the file.
     * @param newFileName The new name for the file.
     * @throws IOException If there are any issues with the file
     */
    private void editObjLine(Path filePath, String newFileName)
            throws IOException {
        ArrayList<String> fileContent = new ArrayList<>(
                Files.readAllLines(filePath, StandardCharsets.UTF_8));

        for (int i = 0; i < fileContent.size(); i++) {
            if (fileContent.get(i).equals("mtllib Model.mtl")) {
                fileContent.set(i, "mtllib " + newFileName + ".mtl");
                break;
            }
        }

        Files.write(filePath, fileContent, StandardCharsets.UTF_8);
    }

    /**
     * Launches the frame
     *
     * @param args not used
     */
    public static void main(String[] args) {
        launch(args);
    }
}
