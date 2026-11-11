import { DownloadButton } from '@mms/mms-ui-library/components/DownloadButton';

import { CONVERSATIONS_COLUMNS_NAMES } from '../../../../constants';
import { DEFAULT_VALUE } from '../../config';
import { FieldSection } from '../FieldSection';

import { AttachedFileWithTypes } from '@/components/AttachedFileWithTypes';
import { useAttachedDocumentDownload } from '@/components/Grid/hooks/useAttachedDocumentDownload';
import { getFileNameWithExtension } from '@/helpers/getFileNameWithExtension';

import { List, Value } from './styles';
import { DocumentsProps } from './types';

export const Documents = ({ documents }: DocumentsProps) => {
	const { downloadAttachedDocument } = useAttachedDocumentDownload();
	const hasDocuments = Boolean(documents?.length);

	return (
		<FieldSection label={CONVERSATIONS_COLUMNS_NAMES.files} withDivider={false}>
			{hasDocuments ? (
				<List>
					{documents.map(({ fileId, fileName, fileSize, fileType }) => {
						const fileNameWithExtension = getFileNameWithExtension(
							fileName,
							fileType
						);

						return (
							<AttachedFileWithTypes
								hasMaxWidth={false}
								key={fileId}
								fileName={fileNameWithExtension || DEFAULT_VALUE}
								fileSize={fileSize}
								fileControl={
									<DownloadButton
										onClick={downloadAttachedDocument(
											fileId,
											fileNameWithExtension
										)}
									/>
								}
							/>
						);
					})}
				</List>
			) : (
				<Value variant="m-400">{DEFAULT_VALUE}</Value>
			)}
		</FieldSection>
	);
};
