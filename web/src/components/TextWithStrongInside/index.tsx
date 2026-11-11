import type { TextWithStrongInsideProps } from './types';

export function TextWithStrongInside({
	before,
	strong,
	after,
}: TextWithStrongInsideProps) {
	return (
		<span>
			{before}
			<strong style={{ fontWeight: 600 }}>{` ${strong} `}</strong>
			{after}
		</span>
	);
}
