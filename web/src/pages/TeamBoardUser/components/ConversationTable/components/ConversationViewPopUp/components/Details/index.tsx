import { CONVERSATIONS_COLUMNS_NAMES } from '../../../../constants';
import { DETAIL_FIELDS } from '../../config';
import { PropsWithConversationData } from '../../types';

import { DetailField } from './DetailField';
import { Wrapper } from './styles';

export const Details = ({ conversation }: PropsWithConversationData) => (
	<Wrapper>
		{DETAIL_FIELDS.map((key) => (
			<DetailField
				key={key}
				label={CONVERSATIONS_COLUMNS_NAMES[key]}
				value={conversation[key]?.toString()}
			/>
		))}
	</Wrapper>
);
