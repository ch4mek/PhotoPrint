import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { lazy, Suspense, useState } from 'react';

import { getPayslipFileUri } from '@/constants/queryPaths';

import { PayslipLinkTypography } from './styles';
import { PayslipLinkProps } from './types';

const DocViewer = lazy(async () => ({
	default: (await import('@/components/DocViewer')).DocViewer,
}));

export function PayslipLink({ fileName, fileId }: PayslipLinkProps) {
	const [isViewerOpened, setIsViewerOpened] = useState(false);

	const handleOpenPayslip = () => {
		setIsViewerOpened(true);
	};

	const handleClosePayslip = () => setIsViewerOpened(false);

	return (
		<>
			<Tooltip text={fileName} useIfTextCut>
				<PayslipLinkTypography variant="m-400" onClick={handleOpenPayslip}>
					{fileName}
				</PayslipLinkTypography>
			</Tooltip>
			{isViewerOpened && (
				<Suspense fallback={null}>
					<DocViewer
						{...{
							fileId,
							fileName,
						}}
						onClose={handleClosePayslip}
						uri={getPayslipFileUri(fileId)}
					/>
				</Suspense>
			)}
		</>
	);
}
