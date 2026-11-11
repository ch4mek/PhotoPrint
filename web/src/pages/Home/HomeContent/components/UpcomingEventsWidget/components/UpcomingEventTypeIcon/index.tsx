import { PropsWithChildren } from 'react';

import { UpcomingEventTypeIconWrapper } from './styles';
import { UpcomingEventTypeIconProps } from './types';

export function UpcomingEventTypeIcon({
	children,
	...props
}: PropsWithChildren<UpcomingEventTypeIconProps>) {
	return (
		<UpcomingEventTypeIconWrapper {...props}>
			{children}
		</UpcomingEventTypeIconWrapper>
	);
}
