import styled from '@emotion/styled';

import { TabTableWrapper } from '@/components/TabWrapper';
import {
	getMinCertificateGridHeight,
	MIN_EDUCATION_GRID_HEIGHT,
} from '@/pages/TeamBoardUser/constants';

export const EducationTabWrapper = styled(TabTableWrapper)<{
	isGridRestricted: boolean;
	hasAddCertificateButton: boolean;
}>(({ theme, isGridRestricted, hasAddCertificateButton }) => ({
	...(isGridRestricted && {
		display: 'grid',
		gap: `${theme.spaces.m}px`,
		gridTemplateColumns: 'minmax(0px, 1fr)',
		gridTemplateRows: `
    minmax(${MIN_EDUCATION_GRID_HEIGHT}px, 1fr)
    minmax(${getMinCertificateGridHeight(hasAddCertificateButton)}px, 1fr)
  `,
	}),
}));
