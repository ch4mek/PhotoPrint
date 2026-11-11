import styled from '@emotion/styled';

import { DEFAULT_DISABLE_OPACITY } from '@/constants/opacities';

export const ExpandCellWrapper = styled.div<{ disabled?: boolean }>(
	({ disabled }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		height: '100%',
		width: '100%',
		cursor: disabled ? 'default' : 'pointer',
	})
);

export const ExpandChevronButtonWrapper = styled.div<{ disabled?: boolean }>(
	({ disabled }) => ({
		display: 'inline-flex',
		opacity: disabled ? DEFAULT_DISABLE_OPACITY : 1,
	})
);
