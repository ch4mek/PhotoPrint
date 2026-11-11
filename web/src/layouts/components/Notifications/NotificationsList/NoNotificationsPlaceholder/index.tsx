import { Typography } from '@mms/mms-ui-library/components/Typography';
import { EmptyBellIcon } from '@mms/mms-ui-library/icons/base/EmptyBellIcon';

import { NO_NOTIFICATIONS_PLACEHOLDER } from '@/constants/staticText';

import { BellIconWrapper, NoNotificationsPlaceholderWrapper } from './styles';

export function NoNotificationsPlaceholder() {
	return (
		<NoNotificationsPlaceholderWrapper>
			<BellIconWrapper>
				<EmptyBellIcon />
			</BellIconWrapper>
			<Typography variant="m-400">{NO_NOTIFICATIONS_PLACEHOLDER}</Typography>
		</NoNotificationsPlaceholderWrapper>
	);
}
