import styled from '@emotion/styled';

import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

export const FilterControlWrapper = styled('div')(() => ({
	display: 'flex',
	justifySelf: 'flex-end',
	alignItems: 'center',
	border: 'none',
	background: 'transparent',
	outline: '',
	borderWidth: 0,
	cursor: 'pointer',
	padding: 0,

	svg: {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},
}));

export const HeaderCellWithFilterContentWrapper = styled('div')(
	({ theme }) => ({
		height: '100%',
		width: '100%',
		display: 'flex',
		justifyContent: 'space-between',
		position: 'relative',
		alignItems: 'center',
		gap: theme.spaces.xs,
	})
);
