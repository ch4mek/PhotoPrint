import { FileWithName } from '../types';

import { Document } from '@/types/document';

export const getFilesToUpload = (documents: Document[]): FileWithName[] =>
	documents
		.filter((document) => document.file)
		.map((document) => {
			const file = document.file as FileWithName;

			file.fileName = document.fileName;

			return file;
		});
