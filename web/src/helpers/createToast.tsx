import { NewToast } from '@mms/mms-ui-library/components/NewToast';
import { JSX } from 'react';

import { NotificationType } from '@/constants/notifications';

export const createToast = (
	type: NotificationType,
	description: string | JSX.Element,
	toastId?: string | number
) => ({
	message: <NewToast status={type} text={description} onClose={() => {}} />,
	settings: {
		type,
		toastId,
		style: { fontFamily: 'inherit' },
	},
});
