import { useRef } from 'react';

import { AttachButton } from '../AttachButton';
import { AttachedFileWithTypes } from '../AttachedFileWithTypes';

import { attachedAllFileExtension } from '@/constants/fileExtensions';

import {
	DEFAULT_ATTACH_THE_DOCUMENT_LABEL,
	MAX_NUMBER_OF_FILES,
} from './constants';
import { AttachFilesGroupWrapper, AttachFilesWrapper } from './styles';
import { AttachFilesGroupProps } from './types';

export function AttachFilesGroupWithTypes({
	files,
	onAttachedFileRemove,
	onAttachedFileChange,
	validExtensions = attachedAllFileExtension,
	title = DEFAULT_ATTACH_THE_DOCUMENT_LABEL,
	maxNumberOfFiles = MAX_NUMBER_OF_FILES,
	fileTypes,
	isAttachMultiple,
	disabled,
}: AttachFilesGroupProps) {
	const hiddenFileInput = useRef<HTMLInputElement>(null);

	const showAddButton = (files.length || 0) < maxNumberOfFiles;

	return (
		<AttachFilesGroupWrapper>
			{files?.length > 0 && (
				<AttachFilesWrapper>
					{files.map(({ fileName, fileId, file }) => (
						<AttachedFileWithTypes
							key={fileId}
							fileName={fileName}
							file={file}
							hasMaxWidth={false}
							handleRemoveClick={onAttachedFileRemove(fileId)}
						/>
					))}
				</AttachFilesWrapper>
			)}

			{showAddButton && (
				<AttachButton
					onAttachedFileChange={onAttachedFileChange}
					title={title}
					validExtensions={validExtensions}
					ref={hiddenFileInput}
					fileTypes={fileTypes ?? []}
					isAttachMultiple={isAttachMultiple}
					disabled={disabled}
				/>
			)}
		</AttachFilesGroupWrapper>
	);
}
