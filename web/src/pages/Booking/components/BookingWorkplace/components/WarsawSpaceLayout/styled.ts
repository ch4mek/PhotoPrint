import styled from '@emotion/styled';

const WARSAW_SPACE_LAYOUT_HEIGHT = 300;
const WARSAW_SPACE_LAYOUT_WIDTH = 420;

export const WarsawSpaceLayoutWrapper = styled('div')(
	() =>
		({
			position: 'relative',

			margin: 'auto',
			height: WARSAW_SPACE_LAYOUT_HEIGHT,
			width: WARSAW_SPACE_LAYOUT_WIDTH,
		} as const)
);
