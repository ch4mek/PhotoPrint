import { useTheme } from '@emotion/react';
import { Switch } from '@mms/mms-ui-library/components/Switch';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ChangeEvent } from 'react';

import { OPEN_ROWS_TOGGLE_TITLE } from './constants';
import { SwitchWrapper } from './styles';
import { OpenRowsToggleProps } from './types';

export function OpenRowsToggle({ checked, onChange }: OpenRowsToggleProps) {
	const theme = useTheme();

	const handleChange = (
		_event: ChangeEvent<HTMLInputElement>,
		{ checked: isChecked }: { checked: boolean }
	) => {
		onChange(isChecked);
	};

	return (
		<SwitchWrapper>
			<Typography variant="m-400" color={theme.palette['text-black']}>
				{OPEN_ROWS_TOGGLE_TITLE}
			</Typography>
			<Switch checked={checked} onChange={handleChange} />
		</SwitchWrapper>
	);
}
