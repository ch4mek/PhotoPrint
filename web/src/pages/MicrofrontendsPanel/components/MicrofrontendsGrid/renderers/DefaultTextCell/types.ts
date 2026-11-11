import { RenderCellProps } from 'react-data-grid';

import { MicrofrontendsGridRow } from '../../types';

export interface DefaultTextCellProps
	extends RenderCellProps<MicrofrontendsGridRow> {
	field: keyof MicrofrontendsGridRow;
}
