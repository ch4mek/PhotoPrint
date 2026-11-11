import { SquareAttachedFile } from '@/components/SquareAttachedFile';

import { SquareAttachedFilesWrapper } from './styled';
import { AttachedFilesListProps } from './types';

export function AttachedFilesList({
	files,
	onFileRemove,
}: AttachedFilesListProps) {
	if (files.length === 0) {
		return null;
	}

	return (
		<SquareAttachedFilesWrapper>
			{files.map((file) => (
				<SquareAttachedFile
					key={file.fileId}
					file={file}
					handleRemoveClick={() => onFileRemove(file.fileId)}
				/>
			))}
		</SquareAttachedFilesWrapper>
	);
}
