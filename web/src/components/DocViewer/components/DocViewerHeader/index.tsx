import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { DownloadIcon } from '@mms/mms-ui-library/icons/base/DownloadIcon';

import { stopPropagation } from '../../util/stopPropagation';

import { FileTypeIcons } from '@/constants/fileExtensions';
import { DocumentFileTypes } from '@/types/file';

import {
	FileIconWrapper,
	FileNameBadge,
	FileNameTypography,
	ViewerHeaderButton,
	ViewerHeaderWrapper,
} from './styles';
import { DocViewerHeaderProps } from './types';

export function DocViewerHeader({
	fileName,
	onClose,
	onExport,
	fileType = DocumentFileTypes.Pdf,
}: DocViewerHeaderProps) {
	const FileIconComponent = FileTypeIcons[fileType];

	return (
		<ViewerHeaderWrapper onClick={stopPropagation}>
			<ViewerHeaderButton onClick={onExport}>
				<DownloadIcon />
			</ViewerHeaderButton>
			<FileNameBadge>
				<FileIconWrapper>
					<FileIconComponent />
				</FileIconWrapper>
				<FileNameTypography variant="m-400">{fileName}</FileNameTypography>
			</FileNameBadge>
			<ViewerHeaderButton onClick={onClose}>
				<CrossIcon />
			</ViewerHeaderButton>
		</ViewerHeaderWrapper>
	);
}
