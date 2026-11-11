import styled from '@emotion/styled';

const WRAPPER_COLUMN_WIDTH = 200;

export const ControlBarWrapper = styled('div')(({ theme }) => ({
	gap: theme.spaces.s,
	display: 'grid',
	gridTemplateColumns: `repeat(3, ${WRAPPER_COLUMN_WIDTH}px) auto`,
}));
