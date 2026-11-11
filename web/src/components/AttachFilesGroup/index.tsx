import { useRef } from 'react';

import { AttachButton } from '@/components/AttachButton';
import { AttachedFile } from '@/components/AttachedFile';
import {
	DEFAULT_ATTACH_THE_DOCUMENT_LABEL,
	MAX_NUMBER_OF_FILES,
} from '@/components/AttachFilesGroup/constants';
import {
	AttachFilesGroupWrapper,
	AttachFilesWrapper,
} from '@/components/AttachFilesGroup/styles';
import { AttachFilesGroupProps } from '@/components/AttachFilesGroup/types';
import { attachedAllFileExtension } from '@/constants/fileExtensions';

export function AttachFilesGroup({
	files,
	onAttachedFileRemove,
	onAttachedFileChange,
	validExtensions = attachedAllFileExtension,
	title = DEFAULT_ATTACH_THE_DOCUMENT_LABEL,
	maxNumberOfFiles = MAX_NUMBER_OF_FILES,
	fileTypes,
	isAttachMultiple,
	shouldShowFileType = true,
}: AttachFilesGroupProps) {
	const hiddenFileInput = useRef<HTMLInputElement>(null);

	return (
		<AttachFilesGroupWrapper>
			{files?.length > 0 && (
				<AttachFilesWrapper>
					{files.map(({ fileName, fileId, fileType }) => (
						<AttachedFile
							key={fileId}
							handleRemoveClick={onAttachedFileRemove(fileId)}
							{...{ fileName, fileType, shouldShowFileType }}
						/>
					))}
				</AttachFilesWrapper>
			)}

			{(files.length || 0) < maxNumberOfFiles && (
				<AttachButton
					onAttachedFileChange={onAttachedFileChange}
					title={title}
					validExtensions={validExtensions}
					ref={hiddenFileInput}
					fileTypes={fileTypes ?? []}
					isAttachMultiple={isAttachMultiple}
				/>
			)}
		</AttachFilesGroupWrapper>
	);
}
