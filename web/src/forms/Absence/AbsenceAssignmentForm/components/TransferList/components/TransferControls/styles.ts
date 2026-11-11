import styled from '@emotion/styled';

import { ASSIGNMENT_FORM_MOBILE_BREAKPOINT } from '@/forms/Absence/AbsenceAssignmentForm/constants';

import { transferControlsGridAreas } from './constants';

const { allRight, checkedRight, allLeft, checkedLeft } =
	transferControlsGridAreas;

const TOP_WRAPPER_MARGIN = 26;

export const TransferControlsWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gap: theme.spaces.s,
	height: 'fit-content',
	gridTemplateColumns: `1fr`,
	gridTemplateAreas: `
    '${allRight}'
    '${checkedRight}'
    '${checkedLeft}'
    '${allLeft}'
  `,
	marginTop: TOP_WRAPPER_MARGIN,

	[theme.breakpoints.down(ASSIGNMENT_FORM_MOBILE_BREAKPOINT)]: {
		justifyContent: 'center',
		gridTemplateColumns: `1fr 1fr 1fr 1fr`,
		gridTemplateAreas: `
    '${allRight} ${checkedRight} ${checkedLeft} ${allLeft}'
  `,
		marginTop: 0,
	},
}));
