import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';

import { FileTypeIcons } from '@/constants/fileExtensions';
import { getFileType } from '@/helpers/getFileType';

import {
	AttachedFileText,
	AttachedFileTextWrapper,
	AttachedFileWrapper,
} from './styles';
import { BaseFileAttachedProps } from './types';

export function BaseFileAttached({
	fileName,
	fileMetaInfo,
	controls,
	maxWidth,
	minWidth,
	onClick,
}: BaseFileAttachedProps) {
	const theme = useTheme();

	const metaData = useMemo(() => fileMetaInfo.join(', '), [fileMetaInfo]);

	const fileType = getFileType(fileName);
	const FileTypeIcon = fileType && FileTypeIcons[fileType];

	return (
		<AttachedFileWrapper
			{...{ maxWidth, minWidth, onClick }}
			isClickable={!!onClick}
		>
			{FileTypeIcon && <FileTypeIcon />}
			<AttachedFileTextWrapper>
				<Tooltip text={fileName} useIfTextCut>
					<AttachedFileText color={theme.palette['text-black']} variant="m-600">
						{fileName}
					</AttachedFileText>
				</Tooltip>
				<Tooltip text={metaData} useIfTextCut>
					<AttachedFileText variant="m-400" color={theme.palette['text-gray']}>
						{metaData}
					</AttachedFileText>
				</Tooltip>
			</AttachedFileTextWrapper>
			{controls?.()}
		</AttachedFileWrapper>
	);
}
