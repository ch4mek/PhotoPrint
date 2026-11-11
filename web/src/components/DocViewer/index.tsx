import {
	DocViewerRenderers,
	IConfig,
	IDocument,
} from '@cyntler/react-doc-viewer';
import '@cyntler/react-doc-viewer/dist/index.css';
import { Spinner } from '@mms/mms-ui-library/components/Spinner';
import { useMemo, useRef } from 'react';
import { createPortal } from 'react-dom';

import { API_URL } from '@/constants/queryPaths';
import { useAuth } from '@/context/AuthContext';
import { triggerFileDownload } from '@/helpers/triggerFileDownload';
import { usePosition } from '@/hooks/usePosition';
import { useResize } from '@/hooks/useResize';
import { useGetBlobFile } from '@/queries/useGetBlobFile';
import { DocumentFileTypes } from '@/types/file';

import { DocViewerHeader } from './components/DocViewerHeader';
import {
	DocumentViewer,
	Overlay,
	SpinnerContainer,
	ViewerBody,
	ViewerLayout,
} from './styles';
import { DocViewerProps } from './types';
import { calcHeaderPosition } from './util/calcHeaderPosition';
import { stopPropagation } from './util/stopPropagation';

const docViewerConfig: IConfig = {
	pdfVerticalScrollByDefault: true,
	header: {
		disableFileName: true,
		disableHeader: true,
		retainURLParams: false,
	},
	loadingRenderer: {
		overrideComponent: () => (
			<SpinnerContainer>
				<Spinner size="small" />
			</SpinnerContainer>
		),
		showLoadingTimeout: false,
	},
};

export function DocViewer({
	fileId,
	fileName,
	onClose,
	uri,
	fileType = DocumentFileTypes.Pdf,
}: DocViewerProps) {
	const { token } = useAuth();
	const ref = useRef<HTMLDivElement>(null);
	const viewerPosition = usePosition(ref);
	const [windowWidth] = useResize();

	const documents = useMemo<IDocument[]>(
		() => [{ uri: `${API_URL}${uri}`, fileName }],
		[fileId, fileName]
	);

	const { refetch: startDownloadingFile } = useGetBlobFile(uri ?? '', {
		enabled: false,
		onSuccess: (responseData: Blob) => {
			triggerFileDownload(responseData, fileName);
		},
	});

	const handleExport = () => {
		if (uri) {
			startDownloadingFile();
		}
	};

	return createPortal(
		<Overlay>
			<ViewerLayout onClick={onClose}>
				<DocViewerHeader
					fileName={fileName}
					onClose={onClose}
					onExport={handleExport}
					fileType={fileType}
				/>
				<ViewerBody onClick={stopPropagation} ref={ref}>
					<DocumentViewer
						documents={documents}
						prefetchMethod="GET"
						requestHeaders={{
							Authorization: `Bearer ${token}`,
						}}
						pluginRenderers={DocViewerRenderers}
						config={docViewerConfig}
						headerPosition={calcHeaderPosition(windowWidth, viewerPosition)}
					/>
				</ViewerBody>
			</ViewerLayout>
		</Overlay>,
		document.body
	);
}
