import styled from '@emotion/styled';

import { DEFAULT_TAG_HEIGHT } from '@/constants/commonStyles';

const INDENTATION_FROM_TOP_BORDER = 3;

export const Wrapper = styled('div')(({ theme }) => ({
	'& > div': {
		height: `${DEFAULT_TAG_HEIGHT}px`,
		marginTop: `${INDENTATION_FROM_TOP_BORDER}px`,
		marginBottom: `${theme.spaces.xs}px`,
	},

	'& + &': {
		marginLeft: theme.spaces.s,
	},
}));
