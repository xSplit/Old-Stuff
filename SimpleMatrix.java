import java.util.Arrays;
import java.util.Random;
 
public class Matrix {
 
    private int[][] matrix;
    public Matrix(int rows, int columns){
        matrix = new int[rows][columns];
        populatematrix(-100,100);
    }
 
    public Matrix(int[][] matrixArray){
        matrix = new int[matrixArray.length][];
        for (int i = 0; i < matrixArray.length; i++) {
            matrix[i] = Arrays.copyOf(matrixArray[i], matrixArray[i].length);
        }
    }
 
    private void populatematrix(int min, int max){
        Random rand = new Random();
        for(int i = 0; i < matrix.length; i++){
            for(int j = 0; j < matrix[0].length; j++){
                matrix[i][j] = rand.nextInt((max - min) + 1) + min;
            }
        }
    }
 
    public Matrix add(Matrix otherMatrix){
        int[][] newMatrix = new int[matrix.length][matrix[0].length];
        for (int i = 0; i < matrix.length; i++)
            for (int j = 0; j < matrix[0].length; j++)
                newMatrix[i][j] = matrix[i][j] + otherMatrix.matrix[i][j];
        return new Matrix(newMatrix);
    }
 
    public Matrix subtract(Matrix otherMatrix){
        int[][] newMatrix = new int[matrix.length][matrix[0].length];
        for (int i = 0; i < matrix.length; i++)
            for (int j = 0; j < matrix[0].length; j++)
                newMatrix[i][j] = matrix[i][j] - otherMatrix.matrix[i][j];
        return new Matrix(newMatrix);
    }
 
    public Matrix dotProduct(Matrix otherMatrix) {
        int[][] newMatrix = new int[matrix.length][otherMatrix.matrix[0].length];
        for (int i = 0; i < matrix.length; i++)
            for (int j = 0; j < otherMatrix.matrix[0].length; j++)
                for (int k = 0; k < matrix.length; k++)
                    newMatrix[i][j] += matrix[i][k] * otherMatrix.matrix[k][j];
        return new Matrix(newMatrix);
    }
 
    public String getPrintableMatrix()
    {
        String result = "";
        for(int[] row : matrix) {
            result += "[";
            for (int col : row)
                result += String.valueOf(col)+',';
            result = result.substring(0,result.length()-1) + "]\n";
        }
        return result;
    }
}
