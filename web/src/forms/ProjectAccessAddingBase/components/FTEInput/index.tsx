import { Input } from '@mms/mms-ui-library/components/Input';
import { ChangeEvent, ComponentProps } from 'react';

import { changeFTEValue } from './utils';

type FTEInputProps = Omit<ComponentProps<typeof Input>, 'onChange'> & {
	onChange: (value: string) => void;
};

export function FTEInput({ onChange, ...restProps }: FTEInputProps) {
	const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
		const targetValue = event.target.value;

		changeFTEValue(targetValue, onChange);
	};

	return <Input label="FTE" onChange={handleChange} {...restProps} />;
}
