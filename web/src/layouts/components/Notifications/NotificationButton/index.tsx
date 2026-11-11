import { BellIcon } from '@mms/mms-ui-library/icons/base/BellIcon';

import { HEADER_ACTION_BUTTON_STYLES } from '@/layouts/main/constants';

import { Badge, NotificationsButtonWrapper } from './styles';
import { NotificationButtonProps } from './types';

export function NotificationsButton({
	onClick,
	showBadge,
	ref,
}: NotificationButtonProps) {
	return (
		<NotificationsButtonWrapper
			ref={ref}
			color="white"
			onClick={onClick}
			size="small"
			style={HEADER_ACTION_BUTTON_STYLES}
		>
			<BellIcon />
			<Badge showBadge={showBadge} />
		</NotificationsButtonWrapper>
	);
}
