import { useState, useCallback, ChangeEvent } from 'react';

import { DEFAULT_MAXIMUM_ALLOWED_REQUEST_FILE_SIZE_KB } from '@/constants/allowedFileSizes';
import { useAttachDocument } from '@/hooks/useAttachDocument';
import { Document } from '@/types/document';

interface UseRequestDocumentsParams {
	extensions: string[];
	maxCount: number;
}

export const useRequestDocuments = ({
	extensions,
	maxCount,
}: UseRequestDocumentsParams) => {
	const [documents, setDocuments] = useState<Document[]>([]);

	const { handleFileChange: baseHandleFileChange } = useAttachDocument(
		extensions,
		DEFAULT_MAXIMUM_ALLOWED_REQUEST_FILE_SIZE_KB
	);

	const handleFileChange = useCallback(
		(e: ChangeEvent<HTMLInputElement>, currentFilesCount: number) => {
			const selectedFiles = baseHandleFileChange(
				e,
				currentFilesCount,
				maxCount
			);

			if (!selectedFiles) {
				return undefined;
			}

			const newFiles = selectedFiles.map((selectedFile, index) => ({
				fileId: `${currentFilesCount + index}-${selectedFile.name}`,
				fileName: selectedFile.name,
				file: selectedFile,
			}));

			setDocuments((prev) => [...prev, ...newFiles]);

			return newFiles;
		},
		[baseHandleFileChange, maxCount]
	);

	const handleFileRemove = useCallback((fileId: string | number) => {
		setDocuments((prev) => prev.filter((doc) => doc.fileId !== fileId));
	}, []);

	const resetDocuments = useCallback(() => {
		setDocuments([]);
	}, []);

	return {
		documents,
		handleFileChange,
		handleFileRemove,
		resetDocuments,
	};
};
