import styled from '@emotion/styled';

import { TimeSheetBreakpoints } from '../../constants';

const { DESKTOP_MD } = TimeSheetBreakpoints;

export const AutoselectWrapper = styled('div')(() => ({
	width: 200,
}));

export const FiltersBarWrapper = styled('div')<{
	isEmployeeOrSysAdminRole: boolean;
	fillAvailableWidth?: boolean;
}>(({ theme, isEmployeeOrSysAdminRole, fillAvailableWidth }) => ({
	display: 'flex',
	flexDirection: 'row',
	gap: theme.spaces.s,
	flexWrap: 'wrap',
	...(fillAvailableWidth && {
		flex: 1,
		minWidth: 0,
	}),

	[theme.breakpoints.down(DESKTOP_MD)]: !isEmployeeOrSysAdminRole && {
		flexDirection: 'column',
		'& > div': {
			width: 270,
		},
	},
}));

export const FilterDropDownWithArrowsWrapper = styled('div')<{
	fillAvailableWidth?: boolean;
}>(({ theme, fillAvailableWidth }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: `${theme.spaces.s}px`,
	...(fillAvailableWidth && {
		flex: 1,
		minWidth: 0,
	}),
}));

export const ResetButtonWrapper = styled('div')<{
	isEmployeeOrSysAdminRole: boolean;
}>(
	({ theme, isEmployeeOrSysAdminRole }) =>
		!isEmployeeOrSysAdminRole && {
			[theme.breakpoints.down(DESKTOP_MD)]: {
				display: 'none',
			},
		}
);
