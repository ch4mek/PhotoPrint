import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

const MOBILE_M_BREACKPOINT = 529;
const TABLET_S_BREACKPOINT = 761;
const TABLET_M_BREACKPOINT = 992;
const DEFAULT_COLUMN_COUNT = 4;
const POPUP_TITLE_WIDTH = 164;

export const UserClaimInfo = styled('div')({
	maxWidth: `calc(100% - ${POPUP_TITLE_WIDTH}px)`,
	width: '100%',
	display: 'flex',
	flexWrap: 'nowrap',
	justifyContent: 'flex-start',
});

export const ClaimsWrapper = styled('div')<{ countColumn?: number }>(
	({ countColumn = DEFAULT_COLUMN_COUNT, theme }) => ({
		display: 'grid',
		height: '100%',
		gap: theme.spaces.m,
		gridTemplateColumns: `repeat(${
			countColumn < DEFAULT_COLUMN_COUNT ? countColumn : DEFAULT_COLUMN_COUNT
		},1fr)`,

		[theme.breakpoints.down(TABLET_M_BREACKPOINT)]: {
			gridTemplateColumns: 'repeat(3,1fr)',
		},

		[theme.breakpoints.down(TABLET_S_BREACKPOINT)]: {
			gridTemplateColumns: 'repeat(2,1fr)',
		},
		[theme.breakpoints.down(MOBILE_M_BREACKPOINT)]: {
			gridTemplateColumns: '100%',
		},
	})
);

export const UserNameTitle = styled(Typography)({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	whiteSpace: 'nowrap',
});
