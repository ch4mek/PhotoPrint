import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { useCallback, useContext } from 'react';

import { NotificationsParameteresContext } from '../../context/NotificationsContext';

import { NotificationDissmissProps } from './types';

export function NotificationDissmiss({ id }: NotificationDissmissProps) {
	const { deleteNotification } = useContext(NotificationsParameteresContext);

	const handleCrossClick = useCallback(() => {
		deleteNotification(id);
	}, []);

	return (
		<ActionButton onClick={handleCrossClick} size="medium">
			<CrossIcon />
		</ActionButton>
	);
}
