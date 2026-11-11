import styled from '@emotion/styled';

import {
	GAP_BETWEEN_GRIDS,
	MIN_CERTIFICATE_GRID_HEIGHT,
	MIN_EDUCATION_GRID_HEIGHT,
} from '@/components/UserProfile/constansts';

export const EducationTabWrapper = styled('div')<{
	isHeightRestricted: boolean;
}>(({ isHeightRestricted }) => ({
	flexGrow: 1,
	minHeight: 0,

	...(isHeightRestricted && {
		display: 'grid',
		gap: GAP_BETWEEN_GRIDS,

		gridTemplateColumns: 'minmax(0px, 1fr)',

		gridTemplateRows: `
      minmax(${MIN_EDUCATION_GRID_HEIGHT}px, 1fr)
      minmax(${MIN_CERTIFICATE_GRID_HEIGHT}px, 1fr)
    `,
	}),
}));
