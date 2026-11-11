import { SchemaOf } from 'yup';

import { UnknownObject } from '@/types/common';

export const removeExtraSpaces = <T extends UnknownObject>(
	values: T,
	schema: SchemaOf<T>
) =>
	Object.fromEntries(
		Object.entries(schema.cast(values)).map(([key, value]) =>
			typeof value === 'string'
				? [
						key,
						value
							.split(' ')
							.filter((val) => val)
							.join(' '),
				  ]
				: [key, value]
		)
	);
