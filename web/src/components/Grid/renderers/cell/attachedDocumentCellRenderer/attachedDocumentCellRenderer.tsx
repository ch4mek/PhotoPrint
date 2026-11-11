import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { DynamicHeightWrapper } from '@/components/Grid/components/DynamicHeightCellWrapper';
import { useAttachedDocumentDownload } from '@/components/Grid/hooks/useAttachedDocumentDownload';
import { getFileNameWithExtension } from '@/helpers/getFileNameWithExtension';
import { DocumentRow } from '@/pages/TeamBoardUser/components/ConversationTable/types';

import { AttachedDocumentCell, AttachedDocumentCellWrapper } from './styles';

export const attachedDocumentCellRenderer = <T extends DocumentRow>({
	row,
	column: { idx },
	onRowChange,
}: RenderCellProps<T, unknown>) => {
	const { downloadAttachedDocument } = useAttachedDocumentDownload();
	const { files } = row;

	return (
		<DynamicHeightWrapper row={row} idx={idx} onRowChange={onRowChange}>
			<AttachedDocumentCellWrapper>
				{files &&
					files.map(({ fileName, fileId, fileType }) => {
						const fileNameWithExtension = fileType
							? getFileNameWithExtension(fileName, fileType)
							: fileName;

						return (
							<Tooltip text={fileNameWithExtension} useIfTextCut key={fileId}>
								<AttachedDocumentCell
									canBeClicked={Boolean(fileId)}
									onClick={downloadAttachedDocument(
										fileId,
										fileNameWithExtension
									)}
								>
									{fileNameWithExtension || ''}
								</AttachedDocumentCell>
							</Tooltip>
						);
					})}
			</AttachedDocumentCellWrapper>
		</DynamicHeightWrapper>
	);
};
