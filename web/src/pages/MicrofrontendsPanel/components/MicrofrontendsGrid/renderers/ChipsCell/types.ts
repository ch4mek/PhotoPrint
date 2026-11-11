import { RenderCellProps } from 'react-data-grid';

import { MicrofrontendsGridRow } from '../../types';

import { MicrofrontendsFormArrayKeys } from '@/pages/MicrofrontendsPanel/components/MicrofrontendsForm/types';

export interface ChipsCellProps extends RenderCellProps<MicrofrontendsGridRow> {
	field: MicrofrontendsFormArrayKeys;
}
