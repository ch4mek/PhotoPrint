import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';

import { UpcomingEventTypeIconProps } from './types';

export const UpcomingEventTypeIconWrapper = styled(
	'div'
)<UpcomingEventTypeIconProps>(({ color, background }) => ({
	color,
	background,
	borderRadius: BorderRadiuses.s,
	width: IconSize.M,
	height: IconSize.M,

	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',

	'& >svg': {
		width: IconSize.MS,
		height: IconSize.MS,
	},
}));
