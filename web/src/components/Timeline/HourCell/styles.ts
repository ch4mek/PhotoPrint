import styled from '@emotion/styled';

export const HourCellWrapper = styled('div')<{
	isDisabled: boolean;
	isLast: boolean;
}>(({ theme, isDisabled, isLast }) => ({
	cursor: isDisabled ? 'default' : 'pointer',
	width: '100%',
	height: '100%',
	borderRight: isLast ? 0 : `1px solid ${theme.palette['graphics-divider-1']}`,
	position: 'relative',

	'&::before': {
		content: '""',
		position: 'absolute',
		top: 0,
		bottom: 0,
		left: '50%',
		transform: 'translateX(-50%)',
		borderLeft: `1px dashed ${theme.palette['graphics-divider-1']}`,
	},
}));
