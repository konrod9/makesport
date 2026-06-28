export type ErrorMessage = {
  code: string;
  message: string;
  invalidField?: string | null;
};

export type ErrorType =
  | "Validation"
  | "Not_found"
  | "Failure"
  | "Conflict"
  | "Authentication"
  | "Authorization";

export type ApiError = {
  messages: ErrorMessage[];
  type: ErrorType;
};

export class EnvelopeError extends Error {
  public readonly apiError: ApiError;
  public readonly type: ErrorType;

  constructor(apiError: ApiError) {
    const firstMessage = apiError.messages[0]?.message ?? "Неизвестная ошибка";
    const message =
      apiError.type === "Failure" ? "Внутренняя ошибка сервера" : firstMessage;

    console.log(apiError.type);

    super(message);

    this.name = "EnvelopeError";
    this.apiError = apiError;
    this.type = apiError.type;

    Object.setPrototypeOf(this, EnvelopeError.prototype);
  }

  get messages(): ErrorMessage[] {
    return this.apiError.messages;
  }

  get firstMessage(): string {
    return this.apiError.type === "Failure"
      ? "Внутренняя ошибка сервера"
      : (this.apiError.messages[0]?.message ?? "Неизвестная ошибка");
  }

  getAllMessages(): string {
    return this.apiError.type === "Failure"
      ? "Внутренняя ошибка сервера"
      : this.apiError.messages.map((msg) => msg.message).join(", ");
  }
}

export function isEnvelopeError(error: unknown): error is EnvelopeError {
  return error instanceof EnvelopeError;
}
