import { DocumentFileTypes } from '@/types/file';

const VALID_TYPES = new Set<string>(Object.values(DocumentFileTypes));

export function getFileType(fileName: string): DocumentFileTypes | undefined {
	const ext = fileName.includes('.')
		? fileName.split('.').pop()?.toLowerCase()
		: undefined;

	return ext && VALID_TYPES.has(ext) ? (ext as DocumentFileTypes) : undefined;
}
