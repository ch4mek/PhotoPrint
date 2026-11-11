import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';

import { BYTES_IN_MB } from '@/constants/file';
import { FileTypeIcons } from '@/constants/fileExtensions';
import { IconSize } from '@/constants/iconSizes';
import { DocumentFileTypes } from '@/types/file';

import {
	AttachedFileText,
	FileImage,
	RemoveAttachedFileButton,
	SquareAttachedFileWrapper,
} from './styles';
import { SquareAttachedFileProps } from './types';

const SQUARE_ATTACHED_FILE_TYPE_WIDTH = 18;
const SQUARE_ATTACHED_FILE_TYPE_HEIGHT = 22;

export function SquareAttachedFile({
	file,
	fileControl,
	handleRemoveClick,
}: SquareAttachedFileProps) {
	const theme = useTheme();

	const sizeMB = (
		(file.fileSize || file.file?.size || 0) / BYTES_IN_MB
	).toFixed(2);

	const fileType =
		file.fileName?.split('.')?.pop()?.toLowerCase() || DocumentFileTypes.Png;

	const FileTypeIcon = FileTypeIcons[fileType as DocumentFileTypes];

	if (file.imagePreview) {
		return (
			<SquareAttachedFileWrapper isImage>
				<FileImage src={file.imagePreview} alt={file.fileName} />
				{fileControl || (
					<RemoveAttachedFileButton onClick={handleRemoveClick}>
						<CrossIcon width={IconSize.MXS} height={IconSize.MXS} />
					</RemoveAttachedFileButton>
				)}
			</SquareAttachedFileWrapper>
		);
	}

	return (
		<SquareAttachedFileWrapper>
			<FileTypeIcon
				width={SQUARE_ATTACHED_FILE_TYPE_WIDTH}
				height={SQUARE_ATTACHED_FILE_TYPE_HEIGHT}
			/>

			<Tooltip text={file.fileName} useIfTextCut>
				<AttachedFileText color={theme.palette['text-black']} variant="s-400">
					{file.fileName}
				</AttachedFileText>
			</Tooltip>
			<Tooltip text={`${sizeMB} Mb`} useIfTextCut>
				<AttachedFileText variant="xs-400" color={theme.palette['text-gray']}>
					{sizeMB} Mb
				</AttachedFileText>
			</Tooltip>

			{fileControl || (
				<RemoveAttachedFileButton onClick={handleRemoveClick}>
					<CrossIcon width={IconSize.MXS} height={IconSize.MXS} />
				</RemoveAttachedFileButton>
			)}
		</SquareAttachedFileWrapper>
	);
}
