import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { useCallback, useMemo } from 'react';

import { ATTACHED_FILE_WRAPPER_MAX_WIDTH } from '@/components/AttachedFileWithTypes/constants';
import { BaseFileAttached } from '@/components/BaseFileAttached';

import { AttachedFileWithTypesProps } from './types';
import { convertBytesToMB } from './utils';

export function AttachedFileWithTypes({
	fileName,
	file,
	fileSize,
	handleRemoveClick,
	onClick,
	fileControl,
	hasMaxWidth = true,
	minWidth,
	maxWidth = ATTACHED_FILE_WRAPPER_MAX_WIDTH,
}: AttachedFileWithTypesProps) {
	const { name } = file || {
		name: fileName,
	};

	const fileMetaInfo = useMemo(() => {
		const { size } = file || {
			size: fileSize || 0,
		};
		const sizeMB = convertBytesToMB(size);

		return [`${sizeMB} Mb`];
	}, [file, fileSize]);

	const controls = useCallback(
		() =>
			fileControl || (
				<ActionButton onClick={handleRemoveClick}>
					<CrossIcon />
				</ActionButton>
			),
		[fileControl, handleRemoveClick]
	);

	const maxSelectedWidth = hasMaxWidth ? maxWidth : undefined;

	return (
		<BaseFileAttached
			{...{ fileMetaInfo, controls, onClick, minWidth }}
			fileName={name}
			maxWidth={maxSelectedWidth}
		/>
	);
}
