import { useRef } from 'react';

import { AttachButton } from '@/components/AttachButton';
import { AttachedFile } from '@/components/AttachedFile';

import { AttachFileButtonProps } from './types';

// eslint-disable-next-line import/no-unused-modules
export function AttachFileButton({
	validExtensions,
	title,
	attachedFileName,
	onAttachedFileRemove,
	onAttachedFileChange,
}: AttachFileButtonProps) {
	const hiddenFileInput = useRef<HTMLInputElement>(null);

	if (attachedFileName) {
		return (
			<AttachedFile
				fileName={attachedFileName}
				handleRemoveClick={onAttachedFileRemove}
			/>
		);
	}

	return (
		<AttachButton
			onAttachedFileChange={onAttachedFileChange}
			title={title}
			validExtensions={validExtensions}
			ref={hiddenFileInput}
		/>
	);
}
