import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { zIndexes } from '@/constants/zIndexes';
import { PerformanceReviewWarningProps } from '@/pages/PerformanceReview/components/PerformanceReviewWarning/types';

import { WARNING_POPUP_MESSAGE, POP_UP_TITLE } from './constants';

export function PerformanceReviewWarning({
	handleToggleDialog,
	handleOpenAddingPopup,
}: PerformanceReviewWarningProps) {
	return (
		<PopUp
			title={POP_UP_TITLE}
			onClose={handleToggleDialog}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: { onClick: handleOpenAddingPopup },
				negativeControl: { onClick: handleToggleDialog },
			}}
			zIndex={zIndexes.formPopup}
		>
			<Typography variant="m-400">{WARNING_POPUP_MESSAGE}</Typography>
		</PopUp>
	);
}
