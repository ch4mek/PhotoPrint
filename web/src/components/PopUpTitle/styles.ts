import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { lightTheme } from '@mms/mms-ui-library/theme';

const RIGHT_MARGIN = 16;

export const PopUpTitleWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	color: lightTheme.palette['text-white'],
	maxWidth: `calc(100% - ${RIGHT_MARGIN}px)`,
	width: '100%',
}));

export const PopUpTitleHeader = styled(Typography)({
	textWrap: 'nowrap',

	overflow: 'hidden',
	textOverflow: 'ellipsis',
	maxWidth: '100%',
});
