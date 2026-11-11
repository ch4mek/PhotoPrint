import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { BookingWarningPopupProps } from './types';

export function BookingWarningPopup({
	warning,
	handleClose,
}: BookingWarningPopupProps) {
	const { t: tCommon } = useTranslation('common', {
		keyPrefix: 'modals',
	});

	return (
		<PopUp
			title={tCommon('warningTitle')}
			onClose={handleClose}
			type="approve"
			headerType="warning"
			showCloseButton
		>
			<Typography variant="m-400">{warning}</Typography>
		</PopUp>
	);
}
