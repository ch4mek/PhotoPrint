import styled from '@emotion/styled';

import { EquipmentExportButtonProps } from './types';

export const EquipmentExportButtonWrapper = styled('div')<
	Pick<EquipmentExportButtonProps, 'isAddButtonShown'>
>(({ theme, isAddButtonShown }) => ({
	marginRight: isAddButtonShown ? theme.spaces.s : 0,
}));
