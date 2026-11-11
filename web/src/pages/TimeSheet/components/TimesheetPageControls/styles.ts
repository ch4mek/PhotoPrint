import styled from '@emotion/styled';
import { breakpoints } from '@mms/mms-ui-library/theme';

import { TimeSheetBreakpoints } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import { FiltersWrapperProps } from './types';

const { tablet } = breakpoints;
const { DESKTOP_MD, DESKTOP } = TimeSheetBreakpoints;

const getMobileFiltersJustifyContent = (
	isPersonalModeOfPMO?: boolean,
	shouldRenderSubmitStatusForEmployee?: boolean
) => {
	if (!isPersonalModeOfPMO) {
		return 'flex-end';
	}

	if (shouldRenderSubmitStatusForEmployee) {
		return 'space-between';
	}

	return 'flex-start';
};

export const FiltersWrapper = styled('div')<FiltersWrapperProps>(
	({ theme, isSubmitRendered }) => ({
		display: 'flex',
		width: '100%',
		justifyContent: 'space-between',

		flexDirection: isSubmitRendered ? 'column' : 'row',

		[theme.breakpoints.up(DESKTOP_MD)]: {
			flexDirection: 'row',
			gap: theme.spaces.m,
		},
	})
);

export const FiltersViewWrapper = styled('div')<{
	isPersonalModeOfPMO?: boolean;
	shouldRenderSubmitStatusForEmployee?: boolean;
}>(({ theme, isPersonalModeOfPMO, shouldRenderSubmitStatusForEmployee }) => {
	const isPersonalWeekModeOfPMO =
		isPersonalModeOfPMO && !shouldRenderSubmitStatusForEmployee;

	return {
		display: 'flex',
		justifyContent: 'space-between',
		width: '100%',
		alignItems: 'start',

		[theme.breakpoints.down(DESKTOP)]: {
			gap: theme.spaces.s,
		},

		[theme.breakpoints.down(DESKTOP_MD)]: {
			alignItems: 'center',
			justifyContent: getMobileFiltersJustifyContent(
				isPersonalModeOfPMO,
				shouldRenderSubmitStatusForEmployee
			),
			flexDirection: isPersonalWeekModeOfPMO ? 'row-reverse' : 'row',
			marginLeft: isPersonalWeekModeOfPMO ? theme.spaces.m : 0,
		},
	};
});

export const FiltersGroupWrapper = styled('div')<{
	fillAvailableWidth?: boolean;
}>(({ theme, fillAvailableWidth }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
	...(fillAvailableWidth && {
		flex: 1,
		minWidth: 0,
	}),

	[theme.breakpoints.down(tablet)]: {
		gap: theme.spaces.m,
	},
}));

export const ActionControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const PopoverContentWrapper = styled('div')<{ isEmployee?: boolean }>(
	({ theme, isEmployee }) => ({
		display: 'flex',
		flexDirection: 'column',
		alignItems: 'flex-end',
		gap: theme.spaces.s,
		border: `1px solid ${theme.palette['primary-4']}`,
		borderRadius: BorderRadiuses.m,
		padding: theme.spaces.m,
		backgroundColor: theme.palette['primary-1'],
		boxShadow: theme.namedShadows.dropDownListWrapper,
		[theme.breakpoints.down(tablet)]: {
			marginTop: isEmployee ? -theme.spaces.s : -theme.spaces.xs,
		},
	})
);
