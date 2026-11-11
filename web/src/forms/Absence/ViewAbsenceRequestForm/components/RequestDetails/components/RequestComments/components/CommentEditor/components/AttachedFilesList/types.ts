import { AppFile } from '@/types/file';

export interface AttachedFilesListProps {
	files: AppFile[];
	onFileRemove: (fileId: string | number) => void;
}
