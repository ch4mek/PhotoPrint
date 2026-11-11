import { memo } from 'react';

import { CountValueText, CountValueTitle, CountValueWrapper } from './styles';

function CountValueItemComponent({
	title,
	count,
}: {
	title: string;
	count: number | string;
}) {
	return (
		<CountValueWrapper>
			<CountValueTitle variant="s-400">
				{title}
				{': '}
			</CountValueTitle>

			<CountValueText variant="m-600">{count}</CountValueText>
		</CountValueWrapper>
	);
}

export const CountValueItem = memo(CountValueItemComponent);
