import { Button } from '@mms/mms-ui-library/components/Button';

import { COLOR_PICKER_CLEAR_TITLE } from './constants';
import { ColorPickerControlProps } from './types';

export function ColorPickerControl({ handleClick }: ColorPickerControlProps) {
	return (
		<Button color="secondary" onClick={handleClick}>
			{COLOR_PICKER_CLEAR_TITLE}
		</Button>
	);
}
