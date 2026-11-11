import { ChangeEvent, useState } from 'react';

import { DEFAULT_MINIMUM_ALLOWED_FILE_SIZE_KB } from '@/constants/allowedFileSizes';
import { convertBytesToKB } from '@/helpers/convertFileSize';
import {
	FileCheck,
	filterFilesWithChecks,
} from '@/helpers/filterFilesWithChecks';

interface FileSelect {
	validExtensions: string[];
	maxFileSizeInKB?: number;
	minFileSizeInKB?: number;
	onInvalidExtensionError?: (validExtenstions: string[]) => void;
	onExceededSizeError?: (maxFileSizeInKB: number) => void;
	onInsufficientSizeError?: (minFileSizeInKB: number) => void;
	onExceededCountError?: (maxFilesCount: number, attachedFiles: number) => void;
}

export const useFileSelect = ({
	validExtensions,
	maxFileSizeInKB,
	minFileSizeInKB = DEFAULT_MINIMUM_ALLOWED_FILE_SIZE_KB,
	onInvalidExtensionError,
	onExceededSizeError,
	onInsufficientSizeError,
	onExceededCountError,
}: FileSelect) => {
	const [currentFiles, setCurrentFiles] = useState<Array<File> | null>(null);

	const removeFile = () => {
		setCurrentFiles(null);
	};

	const checkValidExtension = (fileName: string) =>
		validExtensions.includes(fileName?.split('.').pop()?.toLowerCase() || '');

	const handleUploadRawFiles = (
		uploadedFiles: File[],
		currentFilesCount?: number,
		maxFilesCount?: number
	) => {
		const fileChecks: Array<FileCheck> = [];

		if (onInvalidExtensionError) {
			fileChecks.push({
				displayError: () => onInvalidExtensionError(validExtensions),
				patchFiles: (files) =>
					files.filter(({ name }) => checkValidExtension(name)),
				checkFiles: (files) =>
					files.some(({ name }) => !checkValidExtension(name)),
			});
		}

		if (onExceededSizeError && maxFileSizeInKB) {
			fileChecks.push({
				displayError: () => onExceededSizeError(maxFileSizeInKB),
				patchFiles: (files) =>
					files.filter(({ size }) => convertBytesToKB(size) <= maxFileSizeInKB),
				checkFiles: (files) =>
					Math.max(...files.map(({ size }) => convertBytesToKB(size))) >
					maxFileSizeInKB,
			});
		}

		if (onInsufficientSizeError) {
			fileChecks.push({
				displayError: () => onInsufficientSizeError(minFileSizeInKB),
				patchFiles: (files) =>
					files.filter(({ size }) => convertBytesToKB(size) > minFileSizeInKB),
				checkFiles: (files) =>
					Math.min(...files.map(({ size }) => convertBytesToKB(size))) <=
					minFileSizeInKB,
			});
		}

		if (
			onExceededCountError &&
			currentFilesCount !== undefined &&
			maxFilesCount
		) {
			const leftFiles = maxFilesCount - currentFilesCount;
			fileChecks.push({
				displayError: () =>
					onExceededCountError(maxFilesCount, leftFiles > 0 ? leftFiles : 0),
				patchFiles: (files) =>
					files.slice(0, maxFilesCount - currentFilesCount),
				checkFiles: (files) => files.length + currentFilesCount > maxFilesCount,
			});
		}

		const correctUploadedFiles = filterFilesWithChecks(
			uploadedFiles,
			fileChecks
		);
		setCurrentFiles(correctUploadedFiles ?? null);

		return correctUploadedFiles;
	};

	const handleFileChange = (
		event: ChangeEvent<HTMLInputElement>,
		currentFilesCount?: number,
		maxFilesCount?: number
	) => {
		if (typeof event.target.files === 'object' && event.target.files) {
			const uploadedFiles = Array.from(event.target.files);

			const targetInput = event.currentTarget;
			targetInput.value = '';

			if (!uploadedFiles.length) {
				return;
			}

			return handleUploadRawFiles(
				uploadedFiles,
				currentFilesCount,
				maxFilesCount
			);
		}
	};

	return {
		files: currentFiles,
		handleFileChange,
		removeFile,
		handleUploadRawFiles,
	};
};
