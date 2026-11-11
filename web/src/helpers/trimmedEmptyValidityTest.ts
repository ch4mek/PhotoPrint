import type { TestContext, TestFunction } from 'yup/lib/util/createValidation';

type TrimmedEmptyValidityTest = TestFunction<string | null | undefined>;
type ContextWithOriginal = TestContext & { originalValue?: unknown };

export const trimmedEmptyValidityTest: TrimmedEmptyValidityTest = (
	value,
	context
) => {
	const valueBeforeTransform = (context as ContextWithOriginal).originalValue;

	if (valueBeforeTransform && !value) {
		return false;
	}

	return true;
};
