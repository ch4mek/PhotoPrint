import { Document } from '@/types/document';

export const getFilesToUpload = (fileGroups: Document[]): File[] =>
	fileGroups.reduce<File[]>((acc, uploadedFile) => {
		if (uploadedFile.file instanceof File) {
			acc.push(uploadedFile.file);
		}

		return acc;
	}, []);
