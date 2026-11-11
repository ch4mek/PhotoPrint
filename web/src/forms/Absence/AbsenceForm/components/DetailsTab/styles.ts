import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const SPECIFIC_LINE_HEIGHT = 18;
const MOBILE_BREAKPOINT = 433;

export const CheckboxLabel = styled(Typography)({
	lineHeight: `${SPECIFIC_LINE_HEIGHT}px`,
});

export const HiddenSubmitButtonWrapper = styled('button')({
	display: 'none',
});

export const AutoSelectFiledsWrapper = styled('div', {
	shouldForwardProp: (prop) => prop !== 'isSingleColumn',
})<{ isSingleColumn?: boolean }>(({ theme, isSingleColumn }) => ({
	display: 'grid',
	gridTemplateColumns: isSingleColumn ? '1fr' : 'repeat(2, 1fr)',
	gap: `${theme.spaces.l}px ${theme.spaces.s}px`,

	[theme.breakpoints.down(MOBILE_BREAKPOINT)]: {
		gridTemplateColumns: '1fr',
	},
}));
