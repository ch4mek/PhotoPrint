import styled from '@emotion/styled';

import { IconSize } from '@/constants/iconSizes';

const MARGIN_ICON = 2;

export const DraggableControlIconWrapper = styled('div')<{
	iconSize: 'small' | 'large';
	withIconSpace?: boolean;
}>(({ withIconSpace, iconSize }) => {
	const isSmallIcon = iconSize === 'small';

	return {
		height: isSmallIcon ? IconSize.MS : IconSize.M,

		button: {
			width: isSmallIcon ? IconSize.MS : IconSize.M,
			height: isSmallIcon ? IconSize.MS : IconSize.M,
		},

		...(withIconSpace && {
			marginTop: MARGIN_ICON,
		}),
	};
});
