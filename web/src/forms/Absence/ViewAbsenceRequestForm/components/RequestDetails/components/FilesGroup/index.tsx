import { DownloadButton } from '@mms/mms-ui-library/components/DownloadButton';
import { lazy, MouseEvent, Suspense, useState } from 'react';

import { AttachedFileWithTypes } from '@/components/AttachedFileWithTypes';
import { useAttachedDocumentDownload } from '@/components/Grid/hooks/useAttachedDocumentDownload';
import { getFileUri } from '@/constants/queryPaths';
import { getFileType } from '@/helpers/getFileType';

import { FilesGroupWrapper } from './styles';
import { FilesGroupProps } from './types';

const DocViewer = lazy(async () => ({
	default: (await import('@/components/DocViewer')).DocViewer,
}));

export function FilesGroup({ data }: FilesGroupProps) {
	const [openedFileId, setOpenedFileId] = useState<string | null>(null);

	const { downloadAttachedDocument } = useAttachedDocumentDownload();

	const openedFile = data.find(({ fileId }) => fileId === openedFileId);

	return (
		<FilesGroupWrapper>
			{data.map(({ fileId, fileName, fileSize }) => (
				<AttachedFileWithTypes
					hasMaxWidth={false}
					key={fileId}
					{...{ fileName, fileSize }}
					fileControl={
						<DownloadButton
							onClick={(e: MouseEvent<HTMLButtonElement>) => {
								e.stopPropagation();
								downloadAttachedDocument(fileId, fileName)();
							}}
						/>
					}
					onClick={() => setOpenedFileId(fileId)}
				/>
			))}

			{openedFile && (
				<Suspense fallback={null}>
					<DocViewer
						fileId={openedFile.fileId}
						fileName={openedFile.fileName}
						onClose={() => setOpenedFileId(null)}
						uri={getFileUri(openedFile.fileId)}
						fileType={getFileType(openedFile.fileName)}
					/>
				</Suspense>
			)}
		</FilesGroupWrapper>
	);
}
