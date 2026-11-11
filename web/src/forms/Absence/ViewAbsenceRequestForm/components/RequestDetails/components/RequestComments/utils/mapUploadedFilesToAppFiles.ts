import { DocumentResponse } from '@/types/document';
import { AppFile } from '@/types/file';

export function mapUploadedFilesToAppFiles(
	uploadedFiles: DocumentResponse[],
	originalFiles: File[]
): AppFile[] {
	return uploadedFiles.map((res, index) => ({
		fileId: res.fileId,
		fileName: originalFiles[index].name,
		fileSize: originalFiles[index].size,
		fileType: originalFiles[index].type,
	}));
}

export function mapSelectedFilesToAppFiles(
	selectedFiles: File[],
	existingFilesCount: number
): AppFile[] {
	return selectedFiles.map((file, index) => ({
		fileId: `temp-${file.name}-${existingFilesCount + index}`,
		fileName: file.name,
		file,
		fileSize: file.size,
		fileType: file.type,
		imagePreview: file.type.startsWith('image/')
			? URL.createObjectURL(file)
			: null,
	}));
}
